﻿/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
	private bool matchFound = false;

	void Awake() {
		render = GetComponent<SpriteRenderer>();
    }

	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}

	void OnMouseDown() {
		// 1
		if (render.sprite == null || BoardManager.instance.IsShifting) {
			return;
		}

		if (isSelected) { // 2 Is it already selected?
			Deselect();
		} else {
			if (previousSelected == null) { // 3 Is it the first tile selected?
				Select();
			} else {
				if (GetAllAdjacentTiles().Contains(previousSelected.gameObject)) { // 1
					SwapSprite(previousSelected.render); // 2
					previousSelected.ClearAllMatches();
					previousSelected.Deselect();
					ClearAllMatches();
				} else { // 3
					previousSelected.GetComponent<Tile>().Deselect();
					Select();
				}
			}
		}
	}


	public void SwapSprite(SpriteRenderer render2) { // 1
    	if (render.sprite == render2.sprite) { // 2
        	return;
    	}

    	Sprite tempSprite = render2.sprite; // 3
    	render2.sprite = render.sprite; // 4
    	render.sprite = tempSprite; // 5
    	SFXManager.instance.PlaySFX(Clip.Swap); // 6
		GUIManager.instance.MoveCounter--;
	}

	private GameObject GetAdjacent(Vector2 castDir) {
    	RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
    	if (hit.collider != null) {
        	return hit.collider.gameObject;
    	}
    	return null;
	}

	private List<GameObject> GetAllAdjacentTiles() {
    	List<GameObject> adjacentTiles = new List<GameObject>();
    	for (int i = 0; i < adjacentDirections.Length; i++) {
        	adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
    	}
    	return adjacentTiles;
	}

	private List<GameObject> FindMatch(Vector2 castDir) { // 1
    	List<GameObject> matchingTiles = new List<GameObject>(); // 2
    	RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); // 3
    	while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite) { // 4
        	matchingTiles.Add(hit.collider.gameObject);
        	hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
    	}
    	return matchingTiles; // 5	
	}

	private void ClearMatch(Vector2[] paths) // 1
	{
    	List<GameObject> matchingTiles = new List<GameObject>(); // 2
    	for (int i = 0; i < paths.Length; i++) // 3
   		{
        	matchingTiles.AddRange(FindMatch(paths[i]));
    	}
    	if (matchingTiles.Count >= 2) // 4
    	{
        	for (int i = 0; i < matchingTiles.Count; i++) // 5
        	{
            	matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
        	}
        	matchFound = true; // 6
    	}
	}

	public void ClearAllMatches() {
    	if (render.sprite == null)
        	return;

    	ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
    	ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
    	if (matchFound) {
        	render.sprite = null;
        	matchFound = false;
			StopCoroutine(BoardManager.instance.FindNullTiles());
			StartCoroutine(BoardManager.instance.FindNullTiles());
        	SFXManager.instance.PlaySFX(Clip.Clear);
    	}
	}

}