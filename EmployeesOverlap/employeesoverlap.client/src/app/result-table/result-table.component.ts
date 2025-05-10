import { Component, Input } from '@angular/core';
import { EmployeePairOverlap } from '../core/models/employee-pair-overlap.model';

@Component({
  selector: 'app-result-table',
  templateUrl: './result-table.component.html',
  styleUrls: ['./result-table.component.css'],
  standalone: false,
})
export class ResultTableComponent {
  @Input() data: EmployeePairOverlap[] = [];
}
