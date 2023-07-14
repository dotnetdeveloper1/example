import _ from "lodash";
import { ILayoutItem } from "../../state/ILayoutItem";
import { safeParseInt } from "./safeParseInt";

export function sortLayoutItemsById(items: ILayoutItem[]): ILayoutItem[] {
    return _(items)
        .sortBy((item) => safeParseInt(item.id))
        .value();
}
