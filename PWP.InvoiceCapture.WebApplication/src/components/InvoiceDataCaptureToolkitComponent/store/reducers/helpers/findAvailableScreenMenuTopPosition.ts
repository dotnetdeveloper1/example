export function findAvailableScreenMenuTopPosition(
    rootBoundingBox: DOMRect | null,
    componentBoundingBox: DOMRect | null,
    top: number
): number {
    return rootBoundingBox && componentBoundingBox && rootBoundingBox.height < componentBoundingBox.height + top
        ? rootBoundingBox.height - componentBoundingBox.height
        : top;
}
