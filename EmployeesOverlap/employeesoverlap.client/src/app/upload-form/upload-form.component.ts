import { Component } from '@angular/core';
import { EmployeeOverlapService } from '../core/services/employee-overlap.service';
import { EmployeePairOverlap } from '../core/models/employee-pair-overlap.model';

@Component({
  selector: 'app-upload-form',
  templateUrl: './upload-form.component.html',
  styleUrls: ['./upload-form.component.css'],
  standalone: false,
})
export class UploadFormComponent {
  selectedFile: File | null = null;
  overlaps: EmployeePairOverlap[] = [];
  errorMessage: string = '';

  constructor(private overlapService: EmployeeOverlapService) { }

  onFileChange(event: any) {
    const file = event.target.files[0];
    this.selectedFile = file;
  }

  onSubmit() {
    if (!this.selectedFile) return;

    this.overlapService.uploadCsv(this.selectedFile).subscribe({
      next: (data) => {
        this.overlaps = data;
        this.errorMessage = '';
      },
      error: (err) => {
        this.errorMessage = err.message;
        this.overlaps = [];
      }
    });
  }
}
