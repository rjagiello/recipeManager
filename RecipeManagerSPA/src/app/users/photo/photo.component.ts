import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { Photo } from 'src/app/_models/photo';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';
import { LoaderService } from 'src/app/_services/loader.service';

@Component({
  selector: 'app-photo',
  templateUrl: './photo.component.html',
  styleUrls: ['./photo.component.css']
})
export class PhotoComponent implements OnInit {

  @Input() recipeId: number;
  @Input() edit: boolean;
  @Output() getPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;
  url = '';

  constructor(private authService: AuthService,
              private alertify: AlertifyService,
              private router: Router,
              private loader: LoaderService) { }

  ngOnInit() {
    if (this.recipeId) {
      this.url = this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos/' + this.recipeId;
    } else {
      this.url = this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos/';
    }

    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.url,
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };

    this.uploader.onProgressItem = (progress: any) => {
      this.loader.show();
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      this.loader.hide();
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          description: res.description
        };

        if (this.recipeId) {
          if (!this.edit) {
            this.alertify.success('Dodano przepis');
            this.router.navigate(['/przepisy/szczegoly/' + this.recipeId + '/My']);
          } else {
            this.getPhotoChange.emit(photo.url);
          }
        } else {
          this.authService.changeUserPhoto(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        }
      }
    };
  }

}
