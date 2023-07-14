export function isOutOfComponent(elementRef: HTMLDivElement | null, event: MouseEvent): boolean {
    if (!elementRef || !event) {
        return true;
    }

    const elementBoundingBox = elementRef.getBoundingClientRect();

    return (
        !!elementBoundingBox &&
        (event.pageX < elementBoundingBox.left ||
            event.pageX > elementBoundingBox.left + elementBoundingBox.width ||
            event.pageY < elementBoundingBox.top ||
            event.pageY > elementBoundingBox.top + elementBoundingBox.height)
    );
}
