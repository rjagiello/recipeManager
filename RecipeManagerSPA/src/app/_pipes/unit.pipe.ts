import { PipeTransform, Pipe } from '@angular/core';

@Pipe({
    name: 'unit',
    pure: false
})
export class UnitPipe implements PipeTransform {
    transform(value: any) {
        let unit: string;
        switch (value) {
            case 0:
                unit = 'gram';
                break;
            case 1:
                unit = 'szt.';
                break;
            case 2:
                unit = 'ml';
                break;
            case 3:
                unit = 'łyżki';
                break;
            case 4:
                unit = 'łyżeczki';
                break;
            case 5:
                unit = 'szklanki';
                break;
            case 6:
                unit = 'szczypty';
                break;
            // tslint:disable-next-line:quotemark
            case "0":
                unit = 'gram';
                break;
            // tslint:disable-next-line:quotemark
            case "1":
                unit = 'szt.';
                break;
            // tslint:disable-next-line:quotemark
            case "2":
                unit = 'ml';
                break;
            // tslint:disable-next-line:quotemark
            case "3":
                unit = 'łyżki';
                break;
            // tslint:disable-next-line:quotemark
            case "4":
                unit = 'łyżeczki';
                break;
            // tslint:disable-next-line:quotemark
            case "5":
                unit = 'szklanki';
                break;
            // tslint:disable-next-line:quotemark
            case "6":
                unit = 'szczypty';
                break;
            default:
                unit = 'jednostek';
                break;
        }
        return unit;
    }
}
