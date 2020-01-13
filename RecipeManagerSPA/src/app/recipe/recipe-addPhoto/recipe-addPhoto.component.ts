import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'app-recipe-addPhoto',
  templateUrl: './recipe-addPhoto.component.html',
  styleUrls: ['./recipe-addPhoto.component.css']
})
export class RecipeAddPhotoComponent implements OnInit {

  recipeId = 0;

  constructor(private route: ActivatedRoute,
              private router: Router) { }

  ngOnInit() {
      this.recipeId = +this.route.snapshot.paramMap.get('id');
  }

  skipAddingPhoto() {
    this.router.navigate(['/przepisy/szczegoly/' + this.recipeId + '/My']);
  }
}
