import { Item } from "./item";
import { User } from "./user.model";

export interface Equipment {
    id: number,
    item: Item,
    user: User
}