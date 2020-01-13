import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-remin-password',
  templateUrl: './remin-password.component.html',
  styleUrls: ['./remin-password.component.css']
})
export class ReminPasswordComponent implements OnInit {

  @Output() cancelRemind = new EventEmitter();
  reminderForm: FormGroup;

  constructor(private authService: AuthService,
              private alertify: AlertifyService,
              private fb: FormBuilder,
              private router: Router) { }

  ngOnInit() {
    this.createReminderForm();
  }

  createReminderForm() {
    this.reminderForm = this.fb.group({
      email: ['', Validators.required]
    });
  }

  sendReminder() {
    if (this.reminderForm.valid) {
      const remindObject = Object.assign({}, this.reminderForm.value);

      this.authService.sendReminder(remindObject).subscribe(() => {
        this.alertify.success('Wysłano link do zmiany hasła na podany adres e-mail');
      }, error => {
        this.alertify.error(error);
      }, () => {
        this.cancelRemind.emit(false);
      });
    }
  }

  cancel() {
    this.cancelRemind.emit(false);
  }

}
