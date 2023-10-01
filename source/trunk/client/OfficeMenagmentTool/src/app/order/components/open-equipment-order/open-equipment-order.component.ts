import { ChangeDetectorRef, Component, ElementRef, EventEmitter, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ORDER_STATES, OrderState } from 'src/app/shared/constants/order-states';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { CommentFilter } from 'src/app/shared/models/comment-filter.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { NewComment } from 'src/app/shared/models/new-comment.model';
import { Order } from 'src/app/shared/models/order';
import { CommentsService } from '../../services/comments.service';
import { Comment } from 'src/app/shared/models/comment.model';
import { UpdateOrder } from 'src/app/shared/models/update-order';
import { Role } from 'src/app/shared/constants/role';
import { CommentEdit } from 'src/app/shared/models/comment-edit.model';
import { CommentTypes } from 'src/app/shared/constants/comment-types';
import { OrderRequestService } from '../../services/order-request.service';
import { SignalRService } from 'src/app/shared/services/signalR.service';
import { HubConnectionState } from '@microsoft/signalr';
import { SignalREvents } from 'src/app/shared/constants/signalREvents';

@Component({
  selector: 'app-open-equipment-order',
  templateUrl: './open-equipment-order.component.html',
  styleUrls: ['./open-equipment-order.component.scss']
})
export class OpenEquipmentOrderComponent implements OnInit, OnDestroy{
  @ViewChild("orderContainer", { static: true }) orderContainer!: ElementRef;

  commentsUpdateMinimumScroll: number = 0.8;

  order!: Order;
  orderState = OrderState;
  orderStates = ORDER_STATES;

  UserRole = Role;

  newCommentText: string = "";

  formSubmitted!: EventEmitter<ActionResult<any>>;

  selectedState!: OrderState;

  comments: Comment[] = [];
  loadingComments: boolean = true;
  loadedCommentsCount: number = 0;
  commentBatchSize: number = 10;
  totalComments: number = Math.max();

  loadingNewComment: boolean = false;

  commentTypes = CommentTypes;

  public readonly toastKey: string = "toast";

  constructor(
    private dialogConfig: DynamicDialogConfig,
    private orderRequestService: OrderRequestService,
    public storageService: StorageService,
    private commentsService: CommentsService,
    private changeDetectorRef: ChangeDetectorRef,
    private translateService: TranslateService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private signalRService: SignalRService
    ) { }

  ngOnInit(): void {
    if(this.dialogConfig.data?.order !== undefined) {
      this.order = this.dialogConfig.data.order;
      this.selectedState = this.order.state!;
    }
    this.formSubmitted = this.dialogConfig.data.formSubmitted;
    this.loadComments();

    this.signalRService.reopen()
    .then((connection) => {
      connection.on(SignalREvents.NewCommentForOrderRequest + this.order.id, (comment: any) => {
        if(comment.type == CommentTypes.StateChange) {
          this.selectedState = comment.orderState;
          this.order.state = comment.orderState;
          this.formSubmitted.emit({ success: true, data: { action: CRUDActions.Update, order: this.order } });
        }

        this.comments = [comment, ...this.comments];
        this.loadedCommentsCount++;
      })
    });
  }

  ngAfterViewInit(): void {
    (this.orderContainer.nativeElement as HTMLElement).parentElement!.parentElement!.onscroll = () => {
      if(this.eligableForCommentUpdate()) {
        this.loadComments();
      }
    }
  }

  ngOnDestroy(): void {
      let connection = this.signalRService.instance;
      if(connection.state == HubConnectionState.Connected) {
        connection.off(SignalREvents.NewCommentForOrderRequest + this.order.id);
      }
  }

  eligableForCommentUpdate(): boolean {
    return this.getScrollPosition() > this.commentsUpdateMinimumScroll && this.loadedCommentsCount < this.totalComments && !this.loadingComments;
  }

  changeState(): void {
    let updateStateData: UpdateOrder = {
      id: this.order.id,
      description: this.order.description!,
      state: this.selectedState
    };

    this.orderRequestService.updateOrder(updateStateData).subscribe({
      next: (result: ActionResultResponse<string>) => {
        if(result.actionSuccess == true) {
          this.order.state = this.selectedState;
          this.formSubmitted.emit({ success: true, data: { action: CRUDActions.Update, order: this.order } });
        }
      }
    });
  }

  loadComments(): void {
    this.loadingComments = true;

    let commentFilter: CommentFilter = {
      Users: [],
      Orders: [this.order.id],
      GetOrderComments: true,
      CommentsSkipCount: this.loadedCommentsCount,
      CommentsBatchSize: this.commentBatchSize,
      SortOrder: -1
    };

    this.commentsService.getComments(commentFilter).subscribe({
      next: (response: ActionResultResponse<DataPage<Comment>>) => {
        if(response.actionSuccess != true || response.actionData == null) {
          this.loadingComments = false;
        }

        this.totalComments = response.actionData.totalRecords;
        this.loadingComments = false;

        this.loadedCommentsCount += response.actionData.data.length;

        this.comments = [...this.comments, ...response.actionData.data];

        this.changeDetectorRef.detectChanges();
      },
      error: (error: any) => {
        this.loadingComments = false;
        this.comments = [];
        this.loadedCommentsCount = 0;
      }
    });
  }

  public getScrollPosition(): number {
    let scrollPosition = (this.orderContainer.nativeElement as HTMLElement).parentElement!.parentElement!.scrollTop;

    let scrollHeight = (this.orderContainer.nativeElement as HTMLElement).parentElement!.parentElement!.scrollHeight;
    let clientHeight = (this.orderContainer.nativeElement as HTMLElement).parentElement!.parentElement!.clientHeight;
    let maxScrollPosition = scrollHeight - clientHeight;

    return scrollPosition / maxScrollPosition;
  }

  showReportDeleteConfirm(id: number): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.confirmationService.confirm({
        message: translations['OrderAdministration']['DeleteEquipmentOrderConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.orderRequestService.deleteReport(id).subscribe({
            next:(response: any) => {
              this.formSubmitted.emit({ success: true, data: { action: CRUDActions.Delete } });
              this.confirmationService.close();
              this.messageService.add({ severity: 'success', summary: translations['Common']["Success"], detail: translations['OrderAdministration'][response.actionData], key: this.toastKey });
            }
          });
        },
        reject: () => {
          this.confirmationService.close();
        }
      });
    });
  }

  hideCommentEditSection(comment: Comment, commentTextArea: any, actionSection: HTMLDivElement, editSection: HTMLDivElement, commentElement: HTMLElement): void {
    actionSection.style.display = "flex";
    editSection.style.display = "none";
    commentElement.style.display = "block";
    commentTextArea.value = comment.text;
  }

  showCommentEditSection(actionSection: HTMLDivElement, editSection: HTMLDivElement, commentElement: HTMLElement): void {
    actionSection.style.display = "none";
    editSection.style.display = "block";
    commentElement.style.display = "none";
  }

  editComment(comment: Comment, commentTextArea: any, actionSection: HTMLDivElement, editSection: HTMLDivElement, commentElement: HTMLElement): void {
    let commentEdit: CommentEdit = {
      id: comment.id,
      text: commentTextArea.value
    };

    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.commentsService.updateComment(commentEdit).subscribe({
        next: (response: ActionResultResponse<string>) => {
          if(response == null || response.actionSuccess != true) {
            this.messageService.add({ severity: 'error', summary: translations['Common']["Error"], detail: translations['Comments'][response.errors[0]], key: this.toastKey });
          }

          comment.text = commentTextArea.value;
          actionSection.style.display = "flex";
          editSection.style.display = "none";
          commentElement.style.display = "block";
        },
        error: (error: any) => {
          this.messageService.add({ severity: 'error', summary: translations['Common']["Error"], detail: translations['Comments'][error.message], key: this.toastKey });
        }
      });
    });
  }

  showCommentDeleteConfirm(id: number): void {
    this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
      this.confirmationService.confirm({
        message: translations['Comments']['DeleteCommentConfirmationDescription'],
        header: translations['Common']['DeleteConfirmationTitle'],
        icon: 'pi pi-info-circle',
        accept: () => {
          this.commentsService.deleteComment(id).subscribe({
            next: (response: ActionResultResponse<string>) => {
              if(response == null || response.actionSuccess != true) {
                this.confirmationService.close();
                this.messageService.add({ severity: 'error', summary: translations['Common']["Error"], detail: translations['Comments'][response.errors[0]], key: this.toastKey });
              }
              this.comments = this.comments.filter(com => com.id !== id);
              this.loadedCommentsCount--;
              this.changeDetectorRef.detectChanges();
              this.confirmationService.close();
            },
            error: (error: any) => {
              this.messageService.add({ severity: 'error', summary: translations['Common']["Error"], detail: translations['Comments'][error.message], key: this.toastKey });
            }
          });
        },
        reject: () => {
          this.confirmationService.close();
        }
      });
    });
  }

  addComment(): void {
    let newCommentRequest: NewComment = {
      userId: this.storageService.getUserData()!.id,
      orderId: this.order.id,
      text: this.newCommentText
    }

    this.loadingNewComment = true;
    this.commentsService.postComment(newCommentRequest).subscribe({
      next: (response: ActionResultResponse<Comment>) => {
        if(response.actionSuccess == false) {
          this.translateService.getTranslation(this.translateService.currentLang).subscribe(translations => {
            this.messageService.add({ severity: 'error', summary: translations['Common']["Error"], key: this.toastKey });
          });
        }

        this.loadingNewComment = false;
        this.newCommentText = "";
      }
    });
  }
}
