import { Component, EventEmitter } from '@angular/core';
import { FileUpload } from 'primeng/fileupload';
import { UserService } from '../../services/user.service';
import { ActionResult } from 'src/app/shared/models/action-result.model';
import { HttpErrorResponse } from '@angular/common/http';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { Subscription } from 'rxjs';
import { BusyLoaderComponent } from 'src/app/shared/components/busy-loader/busy-loader.component';
import { FileService } from 'src/app/shared/services/file.service';

@Component({
  selector: 'app-add-csv',
  templateUrl: './add-csv.component.html',
  styleUrls: ['./add-csv.component.scss']
})
export class AddCSVComponent {
  filesUploaded!: EventEmitter<ActionResult<any>>;

  public busy!: Subscription;

  uploadedFiles: any[] = [];

  BusyLoaderComponent = BusyLoaderComponent;

  constructor(
    private dialogConfig: DynamicDialogConfig,
    private fileService: FileService, 
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.filesUploaded = this.dialogConfig.data.filesUploaded;
  }

  getCSVTemplate(): void {
    this.userService.getCSVTemplate().subscribe({
      next: (csv: any) => {
        let blob = this.fileService.base64ToBlob(csv, "text/csv");
        this.fileService.downloadFile(blob, "UserTemplate");
      },
      error: (error: HttpErrorResponse) => {
        if(error?.error) {
          this.filesUploaded.emit({ success: false, data: { message: error?.error[0] } });
        }
        else {
          this.filesUploaded.emit({ success: false, data: { } });
        }
      }
    });
  }

  uploadFiles(event: any, fileUploadComponent: FileUpload): void {
    let files: File[] = event?.files ?? [];

    let formData = new FormData();

    for(let file of files) {
      formData.append("files", file);
    }

    this.busy = this.userService.addUsersFromCSV(formData).subscribe({
      next: (result: ActionResultResponse<string>) => {
        fileUploadComponent._files = [];
        if(result.actionSuccess) {
          this.filesUploaded.emit({ success: true, data: { message: result.actionData } });
        }
        else {
          this.filesUploaded.emit({ success: false, data: { message: result.errors[0]}});
        }
      },
      error: (error: HttpErrorResponse) => {
        this.filesUploaded.emit({ success: false, data: { message: error.message } });
      }
    });
  }
}
