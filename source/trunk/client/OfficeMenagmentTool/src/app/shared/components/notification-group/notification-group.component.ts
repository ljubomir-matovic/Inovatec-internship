import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { NotificationService } from '../../services/notification.service';
import { NotificationViewModel } from '../../models/notification.model';
import { HubConnectionState } from '@microsoft/signalr';
import { DataPage } from '../../models/data-page.model';
import { SignalREvents } from '../../constants/signalREvents';
import { SignalRService } from '../../services/signalR.service';

@Component({
  selector: 'app-notification-group',
  templateUrl: './notification-group.component.html',
  styleUrls: ['./notification-group.component.scss']
})
export class NotificationGroupComponent implements OnInit, OnDestroy {

  @Input() showUnread!: boolean;
  @Output() onNewNotification: EventEmitter<any> = new EventEmitter<any>();
  @Output() onNotificationRead: EventEmitter<any> = new EventEmitter<any>();

  notifications: NotificationViewModel[] = [];
  haveMore: boolean = true;

  constructor(private notificationService: NotificationService, private signalRService: SignalRService) {}

  ngOnInit(): void {
    this.notificationService.getNotifications({ IsRead: !this.showUnread }).subscribe({
      next: (response: DataPage<NotificationViewModel>) => {
        this.notifications = response.data;
        this.haveMore = response.more;
      }
    });

    this.signalRService.reopen()
      .then((connection) => {
        if(this.showUnread) {
            connection.on(SignalREvents.NewNotification, (id: number, data: string, description: string, dateCreated: string) => {
              this.notifications.unshift({
                id,
                data,
                dateCreated,
                description,
                isRead: false
              });
              this.onNewNotification.emit();
            });
        }

        connection.on(SignalREvents.NotificationIsRead, (id: number, data: string, description: string, dateCreated: string) => {
          if(this.showUnread) {
            this.onNotificationRead.emit(1);
            this.notifications = this.notifications.filter(notification => notification.id != id);

            if(this.notifications.length < 3 && this.haveMore) {
              this.getMore();
            }
          } else {
            let i;
            for(i = 0; i < this.notifications.length && this.notifications[i].dateCreated > dateCreated; i++);
            this.notifications.splice(i, 0, { id, data, dateCreated, description, isRead: true});
          }
        });

        connection.on(SignalREvents.DeleteNotification, (id: number) => {
            this.notifications = this.notifications.filter(notification => notification.id != id);
          });
    });
  }

  ngOnDestroy(): void {
    let connection = this.signalRService.instance;
    if(connection.state == HubConnectionState.Connected) {
      connection.off(SignalREvents.NotificationIsRead);
      connection.off(SignalREvents.DeleteNotification);
      connection.off(SignalREvents.NewNotification);
    }
  }

  isRead(index: number): void {
    let notification = this.notifications[index];
    this.signalRService.reopen()
    .then((connection) => {
      connection.invoke(SignalREvents.NotificationIsRead, notification.id);
    });
  }

  getMore(): void {
    this.notificationService.getNotifications({ IsRead: !this.showUnread, UpperBound: this.notifications.at(-1)?.id }).subscribe({
      next: (response: DataPage<NotificationViewModel>) => {
        this.notifications = this.notifications.concat(response.data);
        this.haveMore = response.more;
      }
    })
  }
}
