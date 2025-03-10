import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'CommunicationPortal';  
  
  showPortal = false;
  constructor(private accountServices: AccountService) {
  }
  ngOnInit(): void {    
    this.setCurrentUser();
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.accountServices.setCurrentUser(user);
  }

  errorLog(message: string): void {
    console.log(message);
  }
  openMe(){
    this.showPortal = true;
  }
  PrintMe(){
    this.showPortal = false;
    window.print();
  }
  print(){
    this.showPortal = false;
    window.print();
  }
}
