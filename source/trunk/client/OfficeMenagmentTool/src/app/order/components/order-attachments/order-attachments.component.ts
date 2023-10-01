import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { OrderAttachmentService } from '../../services/order-attachment.service';
import { FileUpload } from 'primeng/fileupload';
import { ConfirmationService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { BusyLoaderComponent } from 'src/app/shared/components/busy-loader/busy-loader.component';
import { Subscription } from 'rxjs';
import { FileService } from 'src/app/shared/services/file.service';

@Component({
  selector: 'app-order-attachments',
  templateUrl: './order-attachments.component.html',
  styleUrls: ['./order-attachments.component.scss']
})
export class OrderAttachmentsComponent implements OnInit, OnChanges {
  busy!: Subscription;

  @Input() orderId: number = 0;
  @ViewChild('file') uploadFileComponents!: FileUpload;

  uploadDialogVisible: boolean = false;
  attachments: any[] = [];
  loading: boolean = true;
  readonly confirmDialogKey: string = "orderAttachmnets";

  BusyLoaderComponent = BusyLoaderComponent;

  constructor(
    private fileService: FileService,
    private orderAttachmentService: OrderAttachmentService,
    private confirmationService: ConfirmationService,
    private translateService: TranslateService
  ) {}

    
  ngOnInit(): void {
      this.loadAttachments();
  }

  ngOnChanges(changes: SimpleChanges): void {
      this.loadAttachments();
  }

  loadAttachments(): void {
    this.loading = true;
    this.orderAttachmentService.getAttachmentsForOrder(this.orderId).subscribe({
      next: (attachments: any[]) => {
        this.attachments = attachments;
        this.loading = false;
      }
    })
  }

  uploadFiles(event: any, fileUploadComponent: FileUpload): void {
    let files: File[] = event?.files ?? [];

    let formData = new FormData();

    formData.set("orderId", this.orderId.toString());

    for(let file of files) {
      formData.append("files", file);
    }

    this.busy = this.orderAttachmentService.uploadFiles(formData).subscribe({
      next: () => {
        fileUploadComponent._files = [];
        this.loadAttachments();
        this.uploadDialogVisible = false;
      },
      error: () => {
        
      }
    });

  }

  openFile(id: number): void {
    this.orderAttachmentService.getAttachmentById(id).subscribe(attachment => {
      let blob = this.fileService.base64ToBlob(attachment.content, attachment.contentType);
      this.fileService.downloadFile(blob, attachment.name);
    });
  }

  deleteFile(id: number): void {
    this.confirmationService.confirm({
      header: this.translateService.instant("OrderAdministration.DeleteAttachmentTitle"),
      message: this.translateService.instant("OrderAdministration.DeleteAttachmentDescription"),
      key: this.confirmDialogKey,
      accept: () => {
        this.orderAttachmentService.deleteFile(id).subscribe({
          next: () => {
            this.loadAttachments();
            this.confirmationService.close();
          }
        });
      }
    });
  }

  showDialog(): void {
    this.uploadDialogVisible = true;
  }

  hideDialog(): void {
    this.uploadFileComponents.clear();
    this.uploadDialogVisible = false;
  }
}
