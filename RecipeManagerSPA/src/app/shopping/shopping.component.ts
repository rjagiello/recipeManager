import { Component, OnInit, TemplateRef } from '@angular/core';
import { ShoppingList } from '../_models/shoppingList';
import { AlertifyService } from '../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { Product, ShoppingProduct } from '../_models/product';
import { ShoppingService } from '../_services/shopping.service';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-shopping',
  templateUrl: './shopping.component.html',
  styleUrls: ['./shopping.component.css']
})
export class ShoppingComponent implements OnInit {

  shoppingLists: ShoppingList[];
  modalRef: BsModalRef;
  config = {
    backdrop: true,
    ignoreBackdropClick: true
  };
  products: ShoppingProduct[];

  constructor(private route: ActivatedRoute,
              private modalService: BsModalService,
              private shoppingService: ShoppingService,
              private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.shoppingLists = data.slist;
      this.shoppingLists.forEach(element => {
        element.products = element.products.filter(this.filterProducts);
      });
    }),
    this.getAllProducts();
  }

  filterProducts(item: ShoppingProduct, index: number, array: ShoppingProduct[]) {
    return item.fridgeCount < item.count;
  }

  removeShoppingCard(shoppingList: ShoppingList) {
    this.shoppingLists.splice(this.shoppingLists.findIndex(s => s.id === shoppingList.id), 1);
    this.getAllProducts();
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, this.config);
  }

  hideModal(createdObject: ShoppingList) {
    if (createdObject != null) {
      this.shoppingLists.push(createdObject);
      this.getAllProducts();
    }
    this.modalRef.hide();
  }

  deleteAllLists() {
    this.alertify.confirm('Czy na pewno usunąć wszystskie listy zakupów?', () => {
      this.shoppingService.deleteAllShoppingLists(this.authService.decodedToken.nameid).subscribe(() => {
        this.shoppingLists = [];
        this.getAllProducts();
        this.alertify.warning('Usunięto wszystskie listy zakupów');
      }, error => {
        this.alertify.error(error);
      });
    });
  }

  getAllProducts() {
    this.shoppingService.getAllProducts(this.authService.decodedToken.nameid).subscribe((data) => {
      this.products = data.filter(this.filterProducts);
    }, error => {
      this.alertify.error(error);
    });
  }
}
