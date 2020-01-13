import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { BsDropdownModule, TabsModule, PaginationModule, ButtonsModule, ModalModule, TooltipModule, CollapseModule } from 'ngx-bootstrap';
import { FileUploadModule } from 'ng2-file-upload';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AutoCompleteModule } from 'primeng/autocomplete';
import {ProgressSpinnerModule} from 'primeng/progressspinner';
import {RadioButtonModule} from 'primeng/radiobutton';
import {CardModule} from 'primeng/card';
import {ToolbarModule} from 'primeng/toolbar';
import {ButtonModule} from 'primeng/button';
import {InputTextModule} from 'primeng/inputtext';
import {AccordionModule} from 'primeng/accordion';
import {FieldsetModule} from 'primeng/fieldset';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { AlertifyService } from './_services/alertify.service';
import { AuthGuard } from './_guards/auth.guard';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { RegisterComponent } from './register/register.component';
import { HomeComponent } from './home/home.component';
import { appRoutes } from './routes';
import { RecipeComponent } from './recipe/recipe.component';
import { UserDetailComponent } from './users/user-detail/user-detail.component';
import { UserDetailResolver } from './_resolvers/user-detail.resolver';
import { UserEditComponent } from './users/user-edit/user-edit.component';
import { UserService } from './_services/user.service';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { UserEditResolver } from './_resolvers/user-edit.resolver';
import { PhotoComponent } from './users/photo/photo.component';
import { RecipesResolver } from './_resolvers/recipe.resolver';
import { CallbackPipe } from './_pipes/callback.pipe';
import { RecipeCardComponent } from './recipe/recipe-card/recipe-card.component';
import { RecipeAddComponent } from './recipe/recipe-add/recipe-add.component';
import { RecipeAddPhotoComponent } from './recipe/recipe-addPhoto/recipe-addPhoto.component';
import { RecipeDetailComponent } from './recipe/recipe-detail/recipe-detail.component';
import { RecipeDetailResolver } from './_resolvers/recipe-detail.resolver';
import { UnitPipe } from './_pipes/unit.pipe';
import { RecipeEditComponent } from './recipe/recipe-edit/recipe-edit.component';
import { ShoppingComponent } from './shopping/shopping.component';
import { ShoppingCardComponent } from './shopping/shopping-card/shopping-card.component';
import { RecipeService } from './_services/recipe.service';
import { ShoppingService } from './_services/shopping.service';
import { ShoppingResolver } from './_resolvers/shopping.resolver';
import { FridgeComponent } from './fridge/fridge.component';
import { FridgeService } from './_services/fridge.service';
import { FridgeResolver } from './_resolvers/fridge.resolver';
import { ShoppingAddComponent } from './shopping/shopping-add/shopping-add.component';
import { UserListComponent } from './users/user-list/user-list.component';
import { UserCardComponent } from './users/user-card/user-card.component';
import { UserListResolver } from './_resolvers/user-list.resolver';
import { UserSearchComponent } from './users/user-search/user-search.component';
import { RecipeFriendComponent } from './recipe/recipe-friend/recipe-friend.component';
import { LoadingComponent } from 'src/app/loading/loading.component';
import { LoaderService } from './_services/loader.service';
import { LoaderInterceptor } from './_services/loader.interceptor';
import { ReminPasswordComponent } from './register/remin-password/remin-password.component';

export function tokenGetter() {
   return localStorage.getItem('token');
}

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      RegisterComponent,
      HomeComponent,
      RecipeComponent,
      UserDetailComponent,
      UserEditComponent,
      PhotoComponent,
      CallbackPipe,
      RecipeCardComponent,
      RecipeAddComponent,
      RecipeAddPhotoComponent,
      RecipeDetailComponent,
      UnitPipe,
      RecipeEditComponent,
      ShoppingComponent,
      ShoppingCardComponent,
      FridgeComponent,
      ShoppingAddComponent,
      UserListComponent,
      UserCardComponent,
      UserSearchComponent,
      RecipeFriendComponent,
      LoadingComponent,
      ReminPasswordComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      FileUploadModule,
      JwtModule.forRoot({
         config: {
            tokenGetter,
            whitelistedDomains: ['localhost:5000'],
            blacklistedRoutes: ['localhost:5000/api/auth']
         }
      }),
      RouterModule.forRoot(appRoutes),
      BsDropdownModule.forRoot(),
      BrowserAnimationsModule,
      PaginationModule.forRoot(),
      TabsModule.forRoot(),
      ButtonsModule.forRoot(),
      ModalModule.forRoot(),
      CollapseModule.forRoot(),
      TooltipModule.forRoot(),
      AutoCompleteModule,
      ProgressSpinnerModule,
      RadioButtonModule,
      CardModule,
      ToolbarModule,
      ButtonModule,
      InputTextModule,
      AccordionModule,
      FieldsetModule
   ],
   providers: [
      AuthService,
      AlertifyService,
      UserService,
      RecipeService,
      ShoppingService,
      FridgeService,
      LoaderService,
      { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true },
      AuthGuard,
      PreventUnsavedChanges,
      ErrorInterceptorProvider,
      UserDetailResolver,
      UserEditResolver,
      RecipesResolver,
      RecipeDetailResolver,
      ShoppingResolver,
      FridgeResolver,
      UserListResolver
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
