import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Recipe, RecipeDetail} from 'src/app/_models/recipe';
import { AuthService } from 'src/app/_services/auth.service';
import { RecipeService } from 'src/app/_services/recipe.service';
import { Product } from 'src/app/_models/product';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';
import { FridgeService } from 'src/app/_services/fridge.service';

@Component({
  selector: 'app-recipe-add',
  templateUrl: './recipe-add.component.html',
  styleUrls: ['./recipe-add.component.css']
})
export class RecipeAddComponent implements OnInit {

  modalRef: BsModalRef;
  config = {
    backdrop: true,
    ignoreBackdropClick: true
  };
  recipeForm: FormGroup;
  recipe: RecipeDetail;
  products: Product[] = [];
  product: any = {};
  results: string[];

  constructor(private authService: AuthService,
              private alertify: AlertifyService,
              private recipeService: RecipeService,
              private fridgeService: FridgeService,
              private fb: FormBuilder,
              private modalService: BsModalService,
              private router: Router) { }

  ngOnInit() {
    this.createRecipeForm();
    console.log(this.products.length);
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, this.config);
  }

  createRecipeForm() {
    this.recipeForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(50)]],
      description: ['', Validators.maxLength(80)],
      category: ['0'],
      preparation: ['', [Validators.required, Validators.maxLength(2000)]],
      portions: ['1']
    });
  }

  AddRecipe() {
    this.recipe = Object.assign({}, this.recipeForm.value);

    this.recipe.products = [];
    this.recipe.products.push(...this.products);

    this.recipeService.addRecipe(this.authService.decodedToken.nameid, this.recipe).subscribe((res: Recipe) => {
      this.router.navigate(['/przepisy/dodawanie/zdjecie/' + res.id]);
    }, error => {
      this.alertify.error(error);
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

  search(event) {
    this.fridgeService.searchProducts(this.authService.decodedToken.nameid, event.query).subscribe((data: string[]) => {
      this.results = data;
    }, error => {
      console.log(error);
    });
  }
}
