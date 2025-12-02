import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { 
  TenantProvisioningService, 
  EditionService, 
  BillingPeriod, 
  EditionDto, 
  TenantProvisioningRequestDto 
} from '../../proxy/saas';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-tenant-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tenant-signup.component.html',
  styleUrls: ['./tenant-signup.component.scss']
})
export class TenantSignupComponent implements OnInit {
  signupForm: FormGroup;
  editions: EditionDto[] = [];
  loading = false;
  submitting = false;
  success = false;
  errorMessage = '';
  billingPeriods = [
    { value: BillingPeriod.Monthly, label: 'Monthly' },
    { value: BillingPeriod.Yearly, label: 'Yearly' }
  ];

  constructor(
    private fb: FormBuilder,
    private tenantProvisioningService: TenantProvisioningService,
    private editionService: EditionService,
    private router: Router
  ) {
    this.signupForm = this.fb.group({
      tenantName: ['', [Validators.required, Validators.minLength(3)]],
      adminEmail: ['', [Validators.required, Validators.email]],
      adminUserName: ['', [Validators.required, Validators.minLength(3)]],
      adminPassword: ['', [Validators.required, Validators.minLength(6)]],
      editionId: ['', Validators.required],
      billingPeriod: [BillingPeriod.Monthly, Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadEditions();
  }

  loadEditions(): void {
    this.loading = true;
    this.editionService.getList()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (editions) => {
          this.editions = editions.filter(e => e.isActive);
        },
        error: (error) => {
          this.errorMessage = 'Failed to load editions. Please try again.';
          console.error('Error loading editions:', error);
        }
      });
  }

  getEditionPrice(editionId: string, billingPeriod: string): string {
    const edition = this.editions.find(e => e.id === editionId);
    if (!edition) return '';
    
    const price = billingPeriod === BillingPeriod.Monthly 
      ? edition.monthlyPrice 
      : edition.yearlyPrice;
    
    return `${price.currency} ${price.amount.toFixed(2)}`;
  }

  onSubmit(): void {
    if (this.signupForm.invalid) {
      Object.keys(this.signupForm.controls).forEach(key => {
        this.signupForm.controls[key].markAsTouched();
      });
      return;
    }

    this.submitting = true;
    this.errorMessage = '';

    const request: TenantProvisioningRequestDto = this.signupForm.value;

    this.tenantProvisioningService.provision(request)
      .pipe(finalize(() => this.submitting = false))
      .subscribe({
        next: (result) => {
          if (result.success) {
            this.success = true;
            setTimeout(() => {
              this.router.navigate(['/']);
            }, 3000);
          } else {
            this.errorMessage = result.message;
          }
        },
        error: (error) => {
          this.errorMessage = error.error?.error?.message || 'An error occurred during signup. Please try again.';
          console.error('Signup error:', error);
        }
      });
  }

  getFieldError(fieldName: string): string {
    const control = this.signupForm.get(fieldName);
    if (control?.touched && control?.errors) {
      if (control.errors['required']) return `${fieldName} is required`;
      if (control.errors['email']) return 'Invalid email address';
      if (control.errors['minlength']) return `Minimum length is ${control.errors['minlength'].requiredLength}`;
    }
    return '';
  }
}
