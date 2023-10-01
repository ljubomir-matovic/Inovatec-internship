import { Office } from "./office";

export interface ReportSchedule {
    id: number,
    office: Office,
    scheduleDate: Date,
    isActive: boolean,
    dateCreated: Date
}