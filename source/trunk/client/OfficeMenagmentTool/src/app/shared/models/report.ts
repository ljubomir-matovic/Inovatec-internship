import { Equipment } from "./equipment.model";
import { Office } from "./office";
import { User } from "./user.model";

export interface Report {
    id: number,
    user: User,
    equipment?: Equipment,
    description: string,
    state?: number,
    category: number,
    dateCreated: Date,
    office: Office
}