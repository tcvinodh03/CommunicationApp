import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'CommunicationPortal';
  users: any;
  constructor(private http: HttpClient) {
  }
  ngOnInit(): void {
    this.http.get('http://localhost:5000/api/User').subscribe({
      next: response => this.users = response,
      error: error => this.errorLog(error),
      complete: () => console.log('Request has been completed')
    })
  }
  errorLog(message: string): void {
    console.log(message);
  }
}
