import { ShoppingProduct, Product } from './product';

export interface ShoppingList {
    id: number;
    name: string;
    products: ShoppingProduct[];
}

export interface ShoppingListToAdd {
    name: string;
    products: Product[];
}
