import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ShoppingListToAdd, ShoppingList } from 'src/app/_models/shoppingList';
import { FormGroup, FormBuilder, Validators, FormControl, FormArray } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { ShoppingService } from 'src/app/_services/shopping.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-shopping-add',
  templateUrl: './shopping-add.component.html',
  styleUrls: ['./shopping-add.component.css']
})
export class ShoppingAddComponent implements OnInit {

  @Output() hideModal = new EventEmitter<ShoppingList>();
  shoppingList: ShoppingListToAdd;
  shoppingListForm: FormGroup;

  constructor(private fb: FormBuilder,
              private authService: AuthService,
              private shoppingService: ShoppingService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.createShoppingListForm();
  }

  createShoppingListForm() {
    this.shoppingListForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(50)]],
      products: new FormArray ([], Validators.required)
    });
  }

  addProd() {
    const creds = this.shoppingListForm.controls.products as FormArray;
    creds.push(this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(30)]],
      count: ['', Validators.required],
      unit: ['', Validators.required]
    }));
  }

  deleteProd(index: number) {
    const creds = this.shoppingListForm.controls.products as FormArray;
    creds.removeAt(index);
  }

  trackByFn(index: any, item: any) {
    return index;
  }

  addShoppingList() {
    this.shoppingList = Object.assign({}, this.shoppingListForm.value);

    this.shoppingService.addShoppingList(this.authService.decodedToken.nameid, this.shoppingList).subscribe((res: ShoppingList) => {
      this.alertify.success('Dodano nową listę zakupów');

      this.hideModal.emit(res);
    }, error => {
      this.alertify.error(error);
      this.hideModal.emit(null);
    });
  }
}
