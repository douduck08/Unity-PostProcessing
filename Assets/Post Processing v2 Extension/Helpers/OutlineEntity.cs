using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DouduckLib;

public sealed class OutlineEntityManager : Singleton<OutlineEntityManager> {

    List<OutlineEntity> _entities = new List<OutlineEntity> ();

    public IEnumerable<OutlineEntity> entities {
        get {
            return _entities;
        }
    }

    public void Register (OutlineEntity entity) {
        _entities.Add (entity);
    }

    public void Unregister (OutlineEntity entity) {
        _entities.Remove (entity);
    }
}

[RequireComponent (typeof (Renderer)), ExecuteInEditMode]
public class OutlineEntity : MonoBehaviour {

    Renderer _renderer;
    public new Renderer renderer {
        get {
            if (_renderer == null) {
                _renderer = GetComponent<Renderer> ();
            }
            return _renderer;
        }
    }

    protected void OnEnable () {
        OutlineEntityManager.instance.Register (this);
    }

    protected void OnDisable () {
        OutlineEntityManager.instance.Unregister (this);
    }
}