import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './_guards/auth.guard';
import { UserDetailComponent } from './users/user-detail/user-detail.component';
import { UserDetailResolver } from './_resolvers/user-detail.resolver';
import { UserEditComponent } from './users/user-edit/user-edit.component';
import { UserEditResolver } from './_resolvers/user-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { RecipeComponent } from './recipe/recipe.component';
import { RecipesResolver } from './_resolvers/recipe.resolver';
import { RecipeAddComponent } from './recipe/recipe-add/recipe-add.component';
import { RecipeAddPhotoComponent } from './recipe/recipe-addPhoto/recipe-addPhoto.component';
import { RecipeDetailComponent } from './recipe/recipe-detail/recipe-detail.component';
import { RecipeDetailResolver } from './_resolvers/recipe-detail.resolver';
import { RecipeEditComponent } from './recipe/recipe-edit/recipe-edit.component';
import { ShoppingComponent } from './shopping/shopping.component';
import { ShoppingResolver } from './_resolvers/shopping.resolver';
import { FridgeComponent } from './fridge/fridge.component';
import { FridgeResolver } from './_resolvers/fridge.resolver';
import { UserListComponent } from './users/user-list/user-list.component';
import { UserListResolver } from './_resolvers/user-list.resolver';
import { UserSearchComponent } from './users/user-search/user-search.component';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  { path: '',
      runGuardsAndResolvers: 'always',
      canActivate: [AuthGuard],
      children: [
        { path: 'uzytkownicy/:id', component: UserDetailComponent,
                                   resolve: {user: UserDetailResolver} },
        { path: 'uzytkownik/edycja', component: UserEditComponent,
                                     resolve: {user: UserEditResolver},
                                     canDeactivate: [PreventUnsavedChanges] },
        { path: 'przepisy', component: RecipeComponent,
                                     resolve: {recipes: RecipesResolver}},
        { path: 'przepisy/dodawanie', component: RecipeAddComponent},
        { path: 'przepisy/dodawanie/zdjecie/:id', component: RecipeAddPhotoComponent},
        { path: 'przepisy/szczegoly/:id/:type', component: RecipeDetailComponent,
                                     resolve: {recipe: RecipeDetailResolver}},
        { path: 'przepisy/edycja/:id', component: RecipeEditComponent,
                                     resolve: {recipe: RecipeDetailResolver}},
        { path: 'zakupy', component: ShoppingComponent,
                                     resolve: {slist: ShoppingResolver}},
        { path: 'lodowka', component: FridgeComponent,
                                     resolve: {fridge: FridgeResolver}},
        { path: 'znajomi', component: UserListComponent,
                                   resolve: {users: UserListResolver} },
        { path: 'znajomi/wyszukiwanie', component: UserSearchComponent }
    ]
  },
  { path: '**', redirectTo: '', pathMatch: 'full'}
];

