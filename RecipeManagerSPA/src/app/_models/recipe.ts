import { Product } from './product';

export interface Recipe {
    id: number;
    name: string;
    description: string;
    category: number;
    isCompleteProducts: boolean;
    photoUrl: string;
}

export interface RecipeDetail {
    id: number;
    name: string;
    description: string;
    category: number;
    preparation: string;
    portions: number;
    photoUrl: string;
    products: Product[];
}

export interface RecipeUpdate {
    name: string;
    description: string;
    category: number;
    preparation: string;
    products: Product[];
}
