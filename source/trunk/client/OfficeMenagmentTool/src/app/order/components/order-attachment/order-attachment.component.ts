import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Tooltip } from 'primeng/tooltip';

@Component({
  selector: 'app-order-attachment',
  templateUrl: './order-attachment.component.html',
  styleUrls: ['./order-attachment.component.scss']
})
export class OrderAttachmentComponent {
  @Input() attachment!: any;
  @Output() onDelete: EventEmitter<number> = new EventEmitter<number>();
  @Output() onClick: EventEmitter<number> = new EventEmitter<number>();

  @ViewChild(Tooltip) tooltip!: Tooltip;

  showTooltip() {
    this.tooltip.activate();
  }

  showAttachment(): void {
    this.onClick.emit(this.attachment.id);
  }

  deleteAttachment(event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    this.onDelete.emit(this.attachment.id);
  }
}
