import { CanDeactivate } from '@angular/router';
import { UserEditComponent } from '../users/user-edit/user-edit.component';

export class PreventUnsavedChanges implements CanDeactivate<UserEditComponent> {
    canDeactivate(component: UserEditComponent) {
        if (component.editForm.touched) {
            return confirm('Jesteś pewien, że chcesz kontunować? Wszelkie niezapisane zmiany zostaną utracone');
        }
        return true;
    }
}
