import { Component, Inject } from '@angular/core';
import { InstanceConfigHolderService } from 'ng-busy';

@Component({
  selector: 'app-busy-loader',
  templateUrl: './busy-loader.component.html',
  styleUrls: ['./busy-loader.component.scss']
})
export class BusyLoaderComponent {
  constructor(@Inject('instanceConfigHolder') private instanceConfigHolder: InstanceConfigHolderService) { }

  get message() {
    return this.instanceConfigHolder.config.message;
  }
}
