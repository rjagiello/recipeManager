export interface Product {
    name: string;
    unit: number;
    count: number;
}

export interface ProductEdit {
    id: number;
    name: string;
    unit: number;
    count: number;
}

export interface ShoppingProduct {
    name: string;
    unit: number;
    count: number;
    boughtCount: number;
    fridgeCount: number;
    isInFridge: boolean;
}
