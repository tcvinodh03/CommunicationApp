import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../member/member-edit/member-edit.component';
import { Component, inject } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (Component) => {
  const confirmService = inject(ConfirmService)

  if (Component.editForm?.dirty) {
    return confirmService.confirm();
  }

  return true;
};
