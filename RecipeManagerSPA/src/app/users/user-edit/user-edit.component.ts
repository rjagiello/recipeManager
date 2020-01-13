import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { User } from 'src/app/_models/user';
import { NgForm, FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user-edit.component.css']
})
export class UserEditComponent implements OnInit {

  user: User;
  photoUrl: string;
  editForm: FormGroup;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.touched) {
      $event.returnValue = true;
    }
  }

  constructor(private route: ActivatedRoute,
              private alertify: AlertifyService,
              private userService: UserService,
              private fb: FormBuilder,
              private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data.user;
    });
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
    this.createEditForm();
  }

  createEditForm() {
    this.editForm = this.fb.group({
      username: [this.user.userName, Validators.required],
      email: [this.user.email, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$')],
      password: ['', [Validators.minLength(6), Validators.maxLength(20)]],
      confirmPassword: ['', ]
    }, { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(fg: FormGroup) {
    return fg.get('password').value === fg.get('confirmPassword').value ? null : { missmatch: true };
  }

  updateUser() {
    if (this.editForm.valid) {
      this.user = Object.assign({}, this.editForm.value);

      this.userService.updateUser(this.authService.decodedToken.nameid, this.user)
        .subscribe(next => {
          this.alertify.success('Profil zaktualizowany');
          this.editForm.reset(this.user);
        }, error => {
          this.alertify.error(error);
        });
    }
  }

  updateMainPhoto(photoUrl) {
    this.user.photoUrl = photoUrl;
  }
}
