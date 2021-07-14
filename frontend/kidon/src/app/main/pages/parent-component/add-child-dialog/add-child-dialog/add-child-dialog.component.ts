import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-add-child-dialog',
  templateUrl: './add-child-dialog.component.html',
  styleUrls: ['./add-child-dialog.component.scss']
})
export class AddChildDialogComponent implements OnInit {


  action: string;
  addChildForm: FormGroup;
  dialogTitle: string;

  constructor(
    private _formBuilder: FormBuilder, 
    public matDialogRef: MatDialogRef<AddChildDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private _data: any) {
    this.action = _data.action;

    if (this.action === 'save' ){
      this.dialogTitle = 'Add new child';
    }
    else{
      this.dialogTitle = '';
    }
  }

  ngOnInit(): void {
    this.addChildForm = this._formBuilder.group({
      childEmail: ['', Validators.required],
      childPassword: ['', Validators.required],
      childName: ['', Validators.required],
    });
  }

}
