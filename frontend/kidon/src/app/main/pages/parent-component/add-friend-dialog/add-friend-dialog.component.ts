import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-add-friend-dialog',
  templateUrl: './add-friend-dialog.component.html',
  styleUrls: ['./add-friend-dialog.component.scss']
})
export class AddFriendDialogComponent implements OnInit {

  action: string;
  addFriendForm: FormGroup;
  dialogTitle: string;

  constructor(
    private _formBuilder: FormBuilder, 
    public matDialogRef: MatDialogRef<AddFriendDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private _data: any) {

      this.action = _data.action;
      if (this.action === 'save' ){
        this.dialogTitle = 'Add new friend';
      }
      else{
        this.dialogTitle = '';
      }
    }

  ngOnInit(): void {
    this.addFriendForm = this._formBuilder.group({
      friendEmail: ['', Validators.required],
    });
  }

}
