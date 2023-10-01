import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../../admin/services/user.service';
import { MessageService } from 'primeng/api';
import { HttpErrorResponse } from '@angular/common/http';
import { User } from 'src/app/shared/models/user.model';
import { TranslateService } from '@ngx-translate/core';
import { StorageService } from 'src/app/shared/helpers/storage.service';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.scss']
})
export class EditProfileComponent {
  editProfileForm!: FormGroup;
  sending: boolean = false;
  hasAdminPermission!: boolean;

  get firstName(): AbstractControl {
    return this.editProfileForm.controls['firstName'];
  }

  get lastName(): AbstractControl {
    return this.editProfileForm.controls['lastName'];
  }

  constructor(
    private fb:FormBuilder,
    private userService:UserService,
    private messageService: MessageService,
    private translateService: TranslateService,
    private storageService: StorageService){}

  ngOnInit(): void {
    this.editProfileForm = this.fb.group({
      firstName:["", Validators.required],
      lastName:["", Validators.required ]
    });

    this.getData();
  }

  getData(): void {
    this.userService.getPersonalData().subscribe({
      next: (data: User | null) => {
        this.firstName.setValue(data?.firstName ?? "");
        this.lastName.setValue(data?.lastName ?? "");
      },
      error: () => window.history.back()
    });
  }

  submitForm() {
    this.sending = true;
    this.userService.changePersonalData(this.editProfileForm.value).subscribe({
      next:(msg:any) => {
        let user: User = this.storageService.getUserData() ?? ({} as User);
        user.firstName = this.firstName.value;
        user.lastName = this.lastName.value;
        this.storageService.storeUserData(user);
        this.sending = false;
        this.messageService.add({ severity: 'success', summary: this.translateService.instant('Common.Success'), key: "edit-profile" })
      },
      error: (error: HttpErrorResponse) => {
        this.sending = false;
      }
    });
  }
}
