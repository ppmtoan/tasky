import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { EditionService, EditionDto, CreateEditionDto, UpdateEditionDto } from '../../../proxy/saas';

@Component({
  selector: 'app-editions',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './editions.component.html',
  styleUrls: ['./editions.component.scss']
})
export class EditionsComponent implements OnInit {
  editions: EditionDto[] = [];
  loading = false;
  showModal = false;
  isEdit = false;
  editionForm: FormGroup;
  selectedEdition: EditionDto | null = null;

  constructor(
    private editionService: EditionService,
    private fb: FormBuilder
  ) {
    this.editionForm = this.fb.group({
      name: ['', Validators.required],
      displayName: ['', Validators.required],
      description: ['', Validators.required],
      isActive: [true],
      monthlyAmount: [0, [Validators.required, Validators.min(0)]],
      monthlyCurrency: ['USD', Validators.required],
      yearlyAmount: [0, [Validators.required, Validators.min(0)]],
      yearlyCurrency: ['USD', Validators.required],
      trialDays: [0, [Validators.required, Validators.min(0)]],
      maxUsers: [0, Validators.min(0)],
      storageQuotaGB: [0, Validators.min(0)],
      apiCallLimit: [0, Validators.min(0)]
    });
  }

  ngOnInit(): void {
    this.loadEditions();
  }

  loadEditions(): void {
    this.loading = true;
    this.editionService.getList({}).subscribe({
      next: (response) => {
        this.editions = response.items || [];
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading editions:', error);
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.isEdit = false;
    this.selectedEdition = null;
    this.editionForm.reset({
      isActive: true,
      monthlyCurrency: 'USD',
      yearlyCurrency: 'USD',
      trialDays: 0,
      maxUsers: 0,
      storageQuotaGB: 0,
      apiCallLimit: 0
    });
    this.showModal = true;
  }

  openEditModal(edition: EditionDto): void {
    this.isEdit = true;
    this.selectedEdition = edition;
    this.editionForm.patchValue({
      name: edition.name,
      displayName: edition.displayName,
      description: edition.description,
      isActive: edition.isActive,
      monthlyAmount: edition.monthlyPrice.amount,
      monthlyCurrency: edition.monthlyPrice.currency,
      yearlyAmount: edition.yearlyPrice.amount,
      yearlyCurrency: edition.yearlyPrice.currency,
      trialDays: edition.trialDays,
      maxUsers: edition.featureLimits['MaxUsers'] || 0,
      storageQuotaGB: edition.featureLimits['StorageQuotaGB'] || 0,
      apiCallLimit: edition.featureLimits['ApiCallLimit'] || 0
    });
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editionForm.reset();
    this.selectedEdition = null;
  }

  onSubmit(): void {
    if (this.editionForm.invalid) {
      Object.keys(this.editionForm.controls).forEach(key => {
        this.editionForm.controls[key].markAsTouched();
      });
      return;
    }

    const formValue = this.editionForm.value;
    const featureLimits: Record<string, number> = {};
    
    if (formValue.maxUsers > 0) featureLimits['MaxUsers'] = formValue.maxUsers;
    if (formValue.storageQuotaGB > 0) featureLimits['StorageQuotaGB'] = formValue.storageQuotaGB;
    if (formValue.apiCallLimit > 0) featureLimits['ApiCallLimit'] = formValue.apiCallLimit;

    const editionData = {
      name: formValue.name,
      displayName: formValue.displayName,
      description: formValue.description,
      isActive: formValue.isActive,
      monthlyPrice: {
        amount: formValue.monthlyAmount,
        currency: formValue.monthlyCurrency
      },
      yearlyPrice: {
        amount: formValue.yearlyAmount,
        currency: formValue.yearlyCurrency
      },
      trialDays: formValue.trialDays,
      featureLimits
    };

    if (this.isEdit && this.selectedEdition) {
      this.editionService.update(this.selectedEdition.id, editionData as UpdateEditionDto).subscribe({
        next: () => {
          this.loadEditions();
          this.closeModal();
        },
        error: (error) => console.error('Error updating edition:', error)
      });
    } else {
      this.editionService.create(editionData as CreateEditionDto).subscribe({
        next: () => {
          this.loadEditions();
          this.closeModal();
        },
        error: (error) => console.error('Error creating edition:', error)
      });
    }
  }

  deleteEdition(id: string): void {
    if (confirm('Are you sure you want to delete this edition?')) {
      this.editionService.delete(id).subscribe({
        next: () => this.loadEditions(),
        error: (error) => console.error('Error deleting edition:', error)
      });
    }
  }

  getFeatureLimitKeys(edition: EditionDto): string[] {
    return Object.keys(edition.featureLimits);
  }
}
