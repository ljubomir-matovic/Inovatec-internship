import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, EventEmitter, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ORDER_STATES, OrderState } from 'src/app/shared/constants/order-states';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { StorageService } from 'src/app/shared/helpers/storage.service';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { CRUDActions } from 'src/app/shared/models/crud-actions.model';
import { Comment } from 'src/app/shared/models/comment.model';
import { ConfirmationService, MessageService } from 'primeng/api';
import { CommentFilter } from 'src/app/shared/models/comment-filter.model';
import { CommentsService } from '../../services/comments.service';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { TranslateService } from '@ngx-translate/core';
import { NewComment } from 'src/app/shared/models/new-comment.model';
import { Role } from 'src/app/shared/constants/role';
import { CommentTypes } from 'src/app/shared/constants/comment-types';
import { CommentEdit } from 'src/app/shared/models/comment-edit.model';
import { ReportService } from '../../services/report.service';
import { UpdateReport } from 'src/app/shared/models/update-problem';
import { Report } from 'src/app/shared/models/report';
import { REPORT_CATEGORIES, ReportCategories } from 'src/app/shared/constants/report-categories';
import { HubConnectionState } from '@microsoft/signalr';
import { SignalRService } from 'src/app/shared/services/signalR.service';
import { SignalREvents } from 'src/app/shared/constants/signalREvents';

@Component({
  selector: 'app-open-report',
  templateUrl: './open-report.component.html',
  styleUrls: ['./open-report.component.scss']
})
export class OpenReportComponent implements OnInit, AfterViewInit, OnDestroy{
  @ViewChild("reportContainer", { static: true }) reportContainer!: ElementRef

  commentsUpdateMinimumScroll: number = 0.8;

  report!: Report;
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

  reportCategory = ReportCategories;
  reportCategories = REPORT_CATEGORIES

  public readonly toastKey: string = "toast";

  constructor(
    private dialogConfig: DynamicDialogConfig,
    private reportService: ReportService,
    public storageService: StorageService,
    private commentsService: CommentsService,
    private changeDetectorRef: ChangeDetectorRef,
    private translateService: TranslateService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private signalRService: SignalRService
  ) { }

  ngOnInit(): void {
    if(this.dialogConfig.data?.report !== undefined) {
      this.report = this.dialogConfig.data.report;
      this.selectedState = this.report.state!;
    }
    this.formSubmitted = this.dialogConfig.data.formSubmitted;

    setTimeout(() => {
      this.signalRService.reopen()
      .then((connection) => {
        connection.on(`${SignalREvents.NewCommentForReport}${this.report.id}`, (comment: any) => {
          if(comment.type == CommentTypes.StateChange) {
            this.selectedState = comment.orderState;
            this.report.state = comment.orderState;
            this.formSubmitted.emit({ success: true, data: { action: CRUDActions.Update, report: this.report } });
          }

          this.comments = [comment, ...this.comments];
          this.loadedCommentsCount++;
        })
      });
    }, 500);
    this.loadComments();
  }

  ngOnDestroy(): void {
    let connection = this.signalRService.instance;
      if(connection.state == HubConnectionState.Connected) {
        connection.off(SignalREvents.NewCommentForReport + this.report.id);
      }
  }

  ngAfterViewInit(): void {
    (this.reportContainer.nativeElement as HTMLElement).parentElement!.parentElement!.onscroll = () => {
      if(this.eligableForCommentUpdate()) {
        this.loadComments();
      }
    }
  }

  eligableForCommentUpdate(): boolean {
    return this.getScrollPosition() > this.commentsUpdateMinimumScroll && this.loadedCommentsCount < this.totalComments && !this.loadingComments;
  }

  changeState(): void {
    let updateReportData: UpdateReport = {
      id: this.report.id,
      description: this.report.description!,
      state: this.selectedState
    }

    this.reportService.updateReport(updateReportData).subscribe({
      next: (result: ActionResultResponse<string>) => {
        if(result.actionSuccess == true) {
          this.report.state = this.selectedState;
          this.formSubmitted.emit({ success: true, data: { action: CRUDActions.Update, report: this.report } });
        }
      }
    })
  }

  loadComments(): void {
    this.loadingComments = true;

    let commentFilter: CommentFilter = {
      Users: [],
      Reports: [this.report.id],
      GetReportComments: true,
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
    let scrollPosition = (this.reportContainer.nativeElement as HTMLElement).parentElement!.parentElement!.scrollTop;

    let scrollHeight = (this.reportContainer.nativeElement as HTMLElement).parentElement!.parentElement!.scrollHeight;
    let clientHeight = (this.reportContainer.nativeElement as HTMLElement).parentElement!.parentElement!.clientHeight;
    let maxScrollPosition = scrollHeight - clientHeight;

    return scrollPosition / maxScrollPosition;
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
      reportId: this.report.id,
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
        this.newCommentText="";
      }
    });
  }
}
