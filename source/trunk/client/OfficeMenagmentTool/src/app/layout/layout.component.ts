import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { AppRoutes } from '../shared/constants/routes';
import { TranslateService } from '@ngx-translate/core';
import { Role } from '../shared/constants/role';
import { StorageService } from '../shared/helpers/storage.service';
import { SignalRService } from '../shared/services/signalR.service';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit, AfterViewInit, OnDestroy{
  menuItems!: MenuItem[];

  routeTranslationKey: string = "Routes";
  subscriptions: any[] = [];

  sidebarVisible: boolean = false;

  constructor (
    private translateService: TranslateService,
    private storageService: StorageService,
    private signalRService: SignalRService
  ) { }

  ngOnInit(): void {
    this.signalRService.initialize("signalR/notification");
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

    this.signalRService.instance.stop().catch();
  }

  setTranslations(translations: any): void {
      this.menuItems = [
        {
          label: translations[this.routeTranslationKey]['AdministrationHeader'],
          expanded: true,
          visible: this.storageService.userAuthenticated(Role.HR, Role.Admin),
          items:
          [
            {
              label: translations[this.routeTranslationKey]['UsersLink'],
              routerLink: [AppRoutes.admin.adminDashboard],
              visible: this.storageService.userAuthenticated(Role.HR, Role.Admin),
              icon: "pi pi-fw pi-users",
              command: e => this.sidebarVisible = false
            },
            {
              label: translations[this.routeTranslationKey]['OfficesLink'],
              routerLink: [AppRoutes.shared.officeAdministrationRoute],
              icon: "pi pi-fw pi-building",
              visible: this.storageService.userAuthenticated(Role.Admin, Role.HR),
              command: e => this.sidebarVisible = false
            },
            {
              label: translations[this.routeTranslationKey]['LogsLink'],
              routerLink: [AppRoutes.admin.logsAdministration],
              visible: this.storageService.userAuthenticated(Role.Admin),
              icon: "pi pi-fw pi-server",
              command: e => this.sidebarVisible = false
            },
            {
              label: translations[this.routeTranslationKey]['ReportSchedulesLink'],
              routerLink: [AppRoutes.hr.reportSchedulesRoute],
              visible: this.storageService.userAuthenticated(Role.HR),
              icon: "pi pi-fw pi-calendar",
              command: e => this.sidebarVisible = false
            },
            {
              label: translations[this.routeTranslationKey]['SuppliersLink'],
              routerLink: [AppRoutes.hr.suppliersAdministrationRoute],
              visible: this.storageService.userAuthenticated(Role.HR),
              icon: "pi pi-fw pi-id-card"
            },
            {
              label: translations[this.routeTranslationKey]['CategoriesLink'],
              routerLink: [AppRoutes.shared.categoriesAdministrationRoute],
              visible: this.storageService.userAuthenticated(Role.HR),
              icon: "pi pi-fw pi-tags",
              command: e => this.sidebarVisible = false
            },
            {
              label: translations[this.routeTranslationKey]['SnacksLink'],
              routerLink: [AppRoutes.shared.snacksAdministrationRoute],
              visible: this.storageService.userAuthenticated(Role.HR),
              icon: "pi pi-fw pi-shopping-cart",
              command: e => this.sidebarVisible = false
            },
            {
              label: translations[this.routeTranslationKey]['EquipmentLink'],
              routerLink: [AppRoutes.shared.equipmentAdministrationRoute],
              visible: this.storageService.userAuthenticated(Role.HR),
              icon: "pi pi-fw pi-desktop",
              command: e => this.sidebarVisible = false
            },
            {
              label: translations[this.routeTranslationKey]['EquipmentEvidentationLink'],
              routerLink: [AppRoutes.shared.equipmentEvidentionRoute],
              visible: this.storageService.userAuthenticated(Role.HR),
              icon: "pi pi-fw pi-inbox",
              command: e => this.sidebarVisible = false
            }
          ]
        },
        {
          label: translations[this.routeTranslationKey]['SnacksSubHeader'],
          expanded: true,
          icon: "pi pi-fw pi-shopping-cart",
          items:
          [
            {
              label: translations[this.routeTranslationKey]['OrderSnacksLink'],
              routerLink: [AppRoutes.shared.orderSnacksRoute],
              icon: "pi pi-fw pi-check-square",
              command: e => this.sidebarVisible = false
            },
            {
              label: translations[this.routeTranslationKey]['SnackOrdersLink'],
              routerLink: [AppRoutes.shared.snackOrdersRoute],
              icon: "pi pi-fw pi-list",
              command: e => this.sidebarVisible = false
            },
            {
              label: translations[this.routeTranslationKey]['OrdersHeader'],
              routerLink: [AppRoutes.hr.ordersListingRoute],
              icon: "pi pi-fw pi-list",
              visible: this.storageService.userAuthenticated(Role.HR),
              command: e => this.sidebarVisible = false
            }
          ]
        },
        {
            label: translations[this.routeTranslationKey]['EquipmentSubHeader'],
            expanded: true,
            icon: "pi pi-fw pi-desktop",
            items:
            [
                {
                    label: translations[this.routeTranslationKey]['NewEquipmentOrderLink'],
                    routerLink: [AppRoutes.shared.orderEquipment],
                    icon: "pi pi-fw pi-file",
                    command: e => this.sidebarVisible = false
                },
                {
                    label: translations[this.routeTranslationKey]['EquipmentOrdersLink'],
                    routerLink: [AppRoutes.shared.equipmentOrders],
                    icon: "pi pi-fw pi-list",
                    command: e => this.sidebarVisible = false
                }
            ]
        },
        {
          label: translations[this.routeTranslationKey]['ProblemsHeader'],
          expanded: true,
          items:
          [
            {
              label: translations[this.routeTranslationKey]['NewProblemLink'],
              routerLink: [AppRoutes.shared.reportProblemRoute],
              icon: "pi pi-fw pi-exclamation-circle",
              command: e => this.sidebarVisible = false
            },
            {
              label: translations[this.routeTranslationKey]['ReportedProblemsLink'],
              routerLink: [AppRoutes.shared.reportedProblemsRoute],
              icon: "pi pi-fw pi-list",
              command: e => this.sidebarVisible = false
            }
          ]
        }
      ];
  }
}
