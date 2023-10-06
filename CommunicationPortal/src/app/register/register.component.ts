import { Component,EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
  @Input() userFormHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();

  ngOnInit(): void {

  }
  model: any = {}
  constructor(private _accountService: AccountService) {

    // console.log(this.userFormHomeComponent[0].userName);

  }
  registerUser() {
    this._accountService.registerUser(this.model).subscribe({
      next:response =>{
        console.log(response);
        this.cancel();
      },
      error:error=>console.log(error)
    })
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
