import { Component } from '@angular/core';
import { EquipmentService } from '../../services/equipment.service';

@Component({
  selector: 'app-equipment-evidention',
  templateUrl: './equipment-evidention.component.html',
  styleUrls: ['./equipment-evidention.component.scss']
})
export class EquipmentEvidentionComponent {
  constructor(private equipmentService:EquipmentService) {}

  get selectedUserId(): number | null {
    return this.equipmentService.selectedUserId;
  }

  get assignLoading(): boolean {
    return this.equipmentService.assignLoading;
  }

  get unassignLoading(): boolean {
    return this.equipmentService.unassignLoading;
  }

  assign(): void {
    this.equipmentService.onAssign.emit();
  }

  unassign(): void {
    this.equipmentService.onUnassign.emit();
  }
}
