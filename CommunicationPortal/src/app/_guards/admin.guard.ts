import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  return accountService.currentUser$.pipe(
    map(usr => {
      if (!usr) return false;
      
      if (usr.roles.includes('Admin') || usr.roles.includes('Moderator')) {
        return true;
      }
      else toastr.error('Privilege restricted');
      return false;
    })
  )
};
