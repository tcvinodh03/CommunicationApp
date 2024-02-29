import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  constructor(private objToastr: ToastrService, private router: Router) { }


  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();
    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      //this.objToastr.info(username + 'has Connected');
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUserSource.next([...usernames, username])
      })
    })

    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlineUserSource.next(usernames.filter(x => x !== username))
      })
    })

    this.hubConnection.on('GetOnlineUsers', username => {
      this.onlineUserSource.next(username);
    })

    this.hubConnection.on('NewMessageReceived', ({ userName, knownAs }) => {
      this.objToastr.info(knownAs + ' has sent a new message!click Me!').onTap.pipe(take(1))
        .subscribe({
          next: () => this.router.navigateByUrl('/members/' + userName + '?tab=Messages')
        });
    })
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error));
  }
}
