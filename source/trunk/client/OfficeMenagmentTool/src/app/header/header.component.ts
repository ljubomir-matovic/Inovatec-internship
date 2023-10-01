import { Component, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { StorageService } from '../shared/helpers/storage.service';
import { MenuItem } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { AppRoutes } from '../shared/constants/routes';
import { UserService } from '../admin/services/user.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})

export class HeaderComponent implements AfterViewInit {
  @Input() sidebarVisible!: boolean;
  @Output() sidebarVisibleChange: EventEmitter<boolean> = new EventEmitter<boolean>();

  menuItems: MenuItem[] = [];
  subscriptions: any[] = [];

  constructor(
    private storageService: StorageService,
    private userService: UserService,
    private translateService: TranslateService) {}

  get fullName(): string {
    return this.storageService.getFullNameOfUser();
  }

  ngAfterViewInit(): void {
    this.subscriptions.push(
      this.translateService.onLangChange.subscribe(
        lang => this.setTranslations(lang.translations)));
    this.subscriptions.push(this.translateService.getTranslation(this.translateService.currentLang).subscribe(
      translations => this.setTranslations(translations)));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  setTranslations(translations: any): void {
    this.menuItems = [
      {
        label: translations['Common']["EditProfile"],
        icon: "pi pi-user-edit",
        routerLink: `/${AppRoutes.shared.editProfileRoute}`
      },
      {
        label: translations['Common']["ChangePassword"],
        icon: "bi bi-key-fill",
        routerLink: `/${AppRoutes.shared.changePasswordRoute}`
      },
      {
        separator: true
      }
      ,
      {
        label: translations["Routes"]["MyProblemsLink"],
        icon:"pi pi-info-circle",
        routerLink: `/${AppRoutes.shared.myProblemsRoute}`
      },
      {
        label: translations["Routes"]['MyEquipmentsLink'],
        routerLink: `/${AppRoutes.shared.myEquipmentsRoute}`,
        icon: "pi pi-fw pi-desktop"
      },
      {
        label: translations["Routes"]['MyEquipmentOrdersLink'],
        routerLink: `/${AppRoutes.shared.myEquipmentOrders}`,
        icon: "pi pi-fw pi-file"
      },
      {
        separator: true
      },
      {
        label: translations['Common']['LogoutLabel'],
        icon: "pi pi-sign-out",
        command: () => this.userService.logout(),
      },
    ];
  }
}
