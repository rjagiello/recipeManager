import { Component, OnInit, TemplateRef } from '@angular/core';
import { RecipeDetail, RecipeUpdate, Recipe } from 'src/app/_models/recipe';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { RecipeService } from 'src/app/_services/recipe.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { Product } from 'src/app/_models/product';
import { FridgeService } from 'src/app/_services/fridge.service';

@Component({
  selector: 'app-recipe-edit',
  templateUrl: './recipe-edit.component.html',
  styleUrls: ['./recipe-edit.component.css']
})
export class RecipeEditComponent implements OnInit {

  recipe: RecipeDetail;
  modalRef: BsModalRef;
  config = {
    backdrop: true,
    ignoreBackdropClick: true
  };
  recipeForm: FormGroup;
  recipeToUpdate: RecipeUpdate;
  products: Product[] = [];
  product: any = {};
  results: string[];

  constructor(private route: ActivatedRoute,
              private authService: AuthService,
              private alertify: AlertifyService,
              private recipeService: RecipeService,
              private fridgeService: FridgeService,
              private fb: FormBuilder,
              private modalService: BsModalService,
              private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.recipe = data.recipe;
      this.products = this.recipe.products;
    });
    this.createRecipeForm();
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, this.config);
  }

  createRecipeForm() {
    this.recipeForm = this.fb.group({
      name: [this.recipe.name, Validators.required],
      description: [this.recipe.description, Validators.maxLength(40)],
      category: [this.recipe.category.toString()],
      preparation: [this.recipe.preparation, Validators.required],
      portions: [this.recipe.portions.toString()]
    });
  }

  addProduct(): void {
    if (this.products.some((value: Product, index: number, array: Product[]) => {
      return value.name.toLocaleLowerCase() === this.product.name.toLocaleLowerCase();
    })) {
      this.alertify.error('Już dodałeś ten składnik');
    } else {
      this.products.push(this.product);
    }
    this.product = {};
    this.modalRef.hide();
  }

  deleteProduct(name: string): void {
    this.products.splice(this.products.findIndex(r => r.name === name), 1);
    console.log(this.products);
  }

  updateRecipe() {
    this.recipeToUpdate = Object.assign({}, this.recipeForm.value);

    this.recipeToUpdate.products = [];
    this.recipeToUpdate.products.push(...this.products);

    this.recipeService.updateRecipe(this.authService.decodedToken.nameid, this.recipe.id, this.recipeToUpdate).subscribe(() => {
      this.alertify.success('Zapisano zmiany');
      this.router.navigate(['/przepisy/szczegoly/' + this.recipe.id + '/My']);
    }, error => {
      this.alertify.error(error);
    });
  }

  updatePhoto(photoUrl) {
    this.recipe.photoUrl = photoUrl;
  }

  search(event) {
    this.fridgeService.searchProducts(this.authService.decodedToken.nameid, event.query).subscribe((data: string[]) => {
      this.results = data;
    }, error => {
      console.log(error);
    });
  }
}
