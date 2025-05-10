import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { EmployeePairOverlap } from '../models/employee-pair-overlap.model';
import { catchError, throwError } from 'rxjs';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmployeeOverlapService {
  private apiUrl = '/employee/overlaps';

  constructor(private http: HttpClient) { }

  uploadCsv(file: File): Observable<EmployeePairOverlap[]> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<EmployeePairOverlap[]>(this.apiUrl, formData).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    let msg = 'Unknown error occurred.';
    if (error.error instanceof ErrorEvent) {
      msg = `Client-side error: ${error.error.message}`;
    } else {
      msg = `Server returned code ${error.status}, message: ${error.error}`;
    }
    return throwError(() => new Error(msg));
  }
}
