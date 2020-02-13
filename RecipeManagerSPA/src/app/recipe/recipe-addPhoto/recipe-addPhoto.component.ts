import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'app-recipe-addPhoto',
  templateUrl: './recipe-addPhoto.component.html',
  styleUrls: ['./recipe-addPhoto.component.css']
})
export class RecipeAddPhotoComponent implements OnInit {

  recipeId = 0;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private alertify: AlertifyService) { }

  ngOnInit() {
      this.recipeId = +this.route.snapshot.paramMap.get('id');
  }

  skipAddingPhoto() {
    this.alertify.success('Dodano przepis');
    this.router.navigate(['/przepisy/szczegoly/' + this.recipeId + '/My']);
  }
}
