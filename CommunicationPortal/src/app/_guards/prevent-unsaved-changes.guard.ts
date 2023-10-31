import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../member/member-edit/member-edit.component';
import { Component } from '@angular/core';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (Component) => {
 if(Component.editForm?.dirty){
  return confirm('Are you sure want to continue, Unsaved changes will be lost')
 }
 
  return true;
};
