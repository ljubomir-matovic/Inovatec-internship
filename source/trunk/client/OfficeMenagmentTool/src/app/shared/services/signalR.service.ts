import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { StorageService } from '../helpers/storage.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  protected _instance!: HubConnection;

  get instance(): HubConnection {
    return this._instance;
  }

  constructor(private storageService: StorageService) { }

  initialize(url: string = "/signalR/base"): void {
    let builder = new HubConnectionBuilder();

    if(environment.production) {
      builder = builder.configureLogging(LogLevel.None);
    }
    else {
      builder = builder.configureLogging(LogLevel.Information);
    }
      
    builder = builder.withUrl(`${environment.serverUrl}/${url}`, { accessTokenFactory: () => this.storageService.getToken() ?? "" })
    .withAutomaticReconnect({ nextRetryDelayInMilliseconds: (retryContext) => {
      switch(retryContext.previousRetryCount) {
        case 0:
          return 0;
        case 1:
          return 50;
        case 2:
          return 100;
        case 3:
          return 500;
        case 4:
          return 1000;
        case 6:
          return 2000;
        default:
          return 10000;
      }
    }  });

      this._instance = builder.build();
  }

  async reopen(): Promise<HubConnection> {
    if(this._instance.state == HubConnectionState.Disconnected) {
      await this._instance.start();
    }

    return this._instance;
  }

  get canRemoveListeners(): boolean {
    return this._instance.state == HubConnectionState.Connected;
  }
}
