import { Component, OnInit, TemplateRef } from '@angular/core';
import { Product, ProductEdit } from '../_models/product';
import { Pagination, PaginationResult } from '../_models/pagination';
import { User } from '../_models/user';
import { FridgeService } from '../_services/fridge.service';
import { AlertifyService } from '../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';

@Component({
  selector: 'app-fridge',
  templateUrl: './fridge.component.html',
  styleUrls: ['./fridge.component.css']
})
export class FridgeComponent implements OnInit {

  fridge: ProductEdit[];
  pagination: Pagination;
  user: User = JSON.parse(localStorage.getItem('user'));
  modalRef: BsModalRef;
  config = {
    backdrop: true,
    ignoreBackdropClick: true
  };

  productForm: FormGroup;
  product: ProductEdit;

  text: string;
  results: string[];

  constructor(private fridgeService: FridgeService,
              private alertify: AlertifyService,
              private route: ActivatedRoute,
              private fb: FormBuilder,
              private modalService: BsModalService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.fridge = data.fridge.result;
      this.pagination = data.fridge.pagination;
    }),
    this.createProductForm();
  }

  createProductForm(product?: ProductEdit) {
    if (product) {
      this.productForm = this.fb.group({
        id: [product.id, Validators.required],
        name: [product.name, [Validators.required, Validators.maxLength(30)]],
        unit: [product.unit, Validators.required],
        count: [product.count, Validators.required]
      });
    } else {
      this.productForm = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(30)]],
        unit: [0, Validators.required],
        count: [1, Validators.required]
      });
    }
  }

  loadFridge() {
    this.fridgeService.getFridgeContent(this.user.id,
      this.pagination.currentPage,
      this.pagination.itemsPerPage)
      .subscribe((res: PaginationResult<ProductEdit[]>) => {
        this.fridge = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertify.error(error);
      });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadFridge();
  }

  openModal(template: TemplateRef<any>, id?: number) {
    if (id) {
      const product = this.fridge.find(p => p.id === id);
      this.createProductForm(product);
    }
    this.modalRef = this.modalService.show(template, this.config);
  }

  addProduct() {
    this.product = Object.assign({}, this.productForm.value);
    this.fridgeService.addProduct(this.user.id, this.product).subscribe((res: ProductEdit) => {
      if (res) {
        this.alertify.success('Dodano produkt do lodówki');
        this.modalRef.hide();
        if (this.fridge.length % 12 === 0) {
          this.loadFridge();
        } else {
          this.fridge.push(res);
        }

        this.productForm.reset();
      }
    }, error => {
      this.alertify.error(error);
      this.modalRef.hide();
    });
  }

  deleteProduct(id: number) {
    this.fridgeService.deleteProduct(this.user.id, id).subscribe(() => {
      this.alertify.success('Usunięto produkt');
      this.fridge.splice(this.fridge.findIndex(p => p.id === id), 1);
      if (this.fridge.length % 12 === 0) {
        this.loadFridge();
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  editProduct() {
    this.product = Object.assign({}, this.productForm.value);
    this.fridgeService.updateProduct(this.user.id, this.product.id, this.product).subscribe(() => {
      this.alertify.success('Zaktualizowano produkt');
      this.modalRef.hide();
      this.loadFridge();
      this.productForm.reset();
    }, error => {
      this.alertify.error(error);
      this.modalRef.hide();
    });
  }

  search(event) {
    this.fridgeService.searchProducts(this.user.id, event.query).subscribe((data: string[]) => {
      this.results = data;
    }, error => {
      console.log(error);
    });
  }

  getProduct() {
    if (this.text === '') {
      this.loadFridge();
    } else {
      this.fridgeService.getProductResult(this.user.id, this.text).subscribe((data: ProductEdit) => {
        this.fridge = [];
        this.fridge.push(data);
      }, error => {
        this.alertify.error(error);
      });
    }
  }
}
