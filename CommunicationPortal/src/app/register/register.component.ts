import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();  
  registerForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;
  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18)
  }

  constructor(private _accountService: AccountService, private _toastr: ToastrService,
    private _formBuilder: FormBuilder, private _router: Router) {
  }

  initializeForm() {
    this.registerForm = this._formBuilder.group({
      userName: ['', Validators.required],
      password: ['', [Validators.required, Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
      gender: ['male'],
      KnownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : { notMatching: true }
    }
  }


  registerUser() {    
    const registerValues = {...this.registerForm.value,dateOfBirth:this.getDateOnly(this.registerForm.controls['dateOfBirth'].value)}    
    this._accountService.registerUser(registerValues).subscribe({
      next: response => {
        this._router.navigateByUrl('/members')
      },
      error: error => {
        debugger;
       this.validationErrors=error;
      }
    })
  }
  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob:string |undefined){
    if(!dob)return;
    let newDob = new Date(dob);
    return new Date(newDob.setMinutes(newDob.getMinutes()-newDob.getTimezoneOffset())).toISOString().slice(0,10);
  }
}
