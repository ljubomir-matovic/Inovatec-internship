import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NotificationViewModel } from '../../models/notification.model';
import { NotificationService } from '../../services/notification.service';
import { NotificationType } from '../../constants/notification-types';
import { Router } from '@angular/router';

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss']
})
export class NotificationComponent {
  @Input() notification!: NotificationViewModel;
  @Input() index!: number;
  @Output() onRead: EventEmitter<any> = new EventEmitter<any>();

  constructor(private notificationService: NotificationService, private router: Router) {}

  executeAction(): void {
    let dataObject = JSON.parse(this.notification.data);
    this.changeState();

    switch(dataObject.type as NotificationType) {
      case NotificationType.Url:
        this.router.navigateByUrl(dataObject.url);
        break;
      default:
        break;
    }

    this.notificationService.menuVisible = false;
  }

  changeState(): void {
    if(this.notification.isRead) {
      return;
    }

    this.notificationService.markAsRead(this.notification.id).subscribe({
      next: (actionResult) => {
        if(actionResult.actionSuccess)
          this.onRead.emit(this.notification.id);
      }
    });
  }
}
