import { Order } from "./order";
import { User } from "./user.model";

export interface Comment {
    id: number,
    user: User,
    order: Order,
    text: string,
    orderState: number,
    type: number,
    dateCreated: Date
}