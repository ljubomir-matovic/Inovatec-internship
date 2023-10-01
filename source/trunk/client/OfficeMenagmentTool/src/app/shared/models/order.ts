import { OrderState } from "../constants/order-states";
import { Item } from "./item";
import { Office } from "./office";
import { User } from "./user.model";

export interface Order {
    id: number,
    user: User,
    item?: Item,
    amount?: number,
    description?: string,
    state?: OrderState,
    dateCreated: Date,
    office: Office
}
