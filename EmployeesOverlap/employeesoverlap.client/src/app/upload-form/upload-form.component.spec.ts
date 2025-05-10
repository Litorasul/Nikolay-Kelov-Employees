import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { UploadFormComponent } from './upload-form.component';
import { FormsModule } from '@angular/forms';
import { EmployeeOverlapService } from '../core/services/employee-overlap.service';

describe('UploadFormComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UploadFormComponent],
      imports: [HttpClientTestingModule, FormsModule],
      providers: [EmployeeOverlapService],
    }).compileComponents();
  });

  it('should create the component', () => {
    const fixture = TestBed.createComponent(UploadFormComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });
});
