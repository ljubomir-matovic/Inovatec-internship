import { OnInit, Component, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { NotificationService } from '../../services/notification.service';
import { MessageService } from 'primeng/api';
import { SignalRService } from '../../services/signalR.service';
import { SignalREvents } from '../../constants/signalREvents';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-notification-menu',
  templateUrl: './notification-menu.component.html',
  styleUrls: ['./notification-menu.component.scss']
})
export class NotificationMenuComponent implements OnInit, OnDestroy{
  readonly toastKey: string = "NotificationMenu";

  get menuVisible(): boolean {
    return this.notificationService.menuVisible;
  }

  set menuVisible(value: boolean) {
    this.notificationService.menuVisible = value;
  }

  bodyClickListener!: any;
  unreadNotificationNumber: number = 1;

  constructor(
    private notificationService: NotificationService,
    private messageService: MessageService,
    private changeDetectorRef: ChangeDetectorRef,
    private signalRService: SignalRService,
    private translateService: TranslateService) {}

  ngOnInit(): void {
    let self = this;

    this.bodyClickListener = function (event: MouseEvent) {
      let notificationMenuElement = document.getElementById("notification-menu");

      if(!(event.target == notificationMenuElement
        || ( notificationMenuElement?.contains(event.target as Node)
        && !(event.target as HTMLElement).classList.contains("notification-container"))
        )) {
        self.menuVisible = false;
      }
    };

    document.body.addEventListener("click", this.bodyClickListener);

    this.notificationService.unreadNotificationNumber().subscribe({
      next: (notificationNumber: number) => {
        this.unreadNotificationNumber = notificationNumber;
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  onNewNotification(): void {
    this.unreadNotificationNumber++;
    this.messageService.add({key: this.toastKey, severity: "info", summary: "Info", detail: this.translateService.instant("Notification.NewNotification") });
  }

  ngOnDestroy(): void {
    document.body.removeEventListener("click", this.bodyClickListener);
  }

  showNotificationMenu(): void {
    this.menuVisible = !this.menuVisible;
  }

  onNotificationRead(): void {
    this.unreadNotificationNumber--;
  }

  markAllAsRead(): void {
    if(this.unreadNotificationNumber == 0) {
      return;
    }

    this.signalRService.reopen()
    .then((connection) => {
      connection.invoke(SignalREvents.MarkAllAsRead);
    });
  }
}
