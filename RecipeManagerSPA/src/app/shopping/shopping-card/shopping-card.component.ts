import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { ShoppingList } from 'src/app/_models/shoppingList';
import { ShoppingService } from 'src/app/_services/shopping.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { ShoppingProduct } from 'src/app/_models/product';

@Component({
  selector: 'app-shopping-card',
  templateUrl: './shopping-card.component.html',
  styleUrls: ['./shopping-card.component.css']
})
export class ShoppingCardComponent implements OnInit {

  @Input() shoppingList: ShoppingList;
  @Output() removeShoppingListCard = new EventEmitter<ShoppingList>();

  isCollapsed = true;

  constructor(private shoppingService: ShoppingService,
              private alertify: AlertifyService,
              private authService: AuthService) { }

  ngOnInit() {}

  finishShopping() {
    this.alertify.confirm('Czy na pewno zakończyć zakupy?', () => {
      this.shoppingService.finishShopping(this.authService.decodedToken.nameid, this.shoppingList).subscribe(() => {
        this.removeShoppingListCard.emit(this.shoppingList);
        this.alertify.success('Zakończono zakupy');
      }, error => {
        this.alertify.error(error);
      });
    });
  }

  deleteShoppingList() {
    this.alertify.confirm('Czy na pewno usunąć listę zakupów?', () => {
      this.shoppingService.deleteShoppingList(this.authService.decodedToken.nameid, this.shoppingList.id).subscribe(() => {
        this.removeShoppingListCard.emit(this.shoppingList);
        this.alertify.success('Usunięto listę zakupów');
      }, error => {
        this.alertify.error(error);
      });
    });
  }

}
